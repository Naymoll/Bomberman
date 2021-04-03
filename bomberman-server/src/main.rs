#![feature(proc_macro_hygiene, decl_macro)]

#[macro_use]
extern crate rocket;
extern crate serde;
extern crate serde_json;

use std::ops::Deref;
use std::sync::atomic::{AtomicU32, Ordering};
use std::sync::RwLock;

use rocket::http::Status;
use rocket::response::content;
use rocket::State;

use crate::api_key::ApiKey;
use crate::data::{Lobby, LobbyStatus, Player, PlayerStatus};
use crate::map::Map;

mod api_key;
mod data;
mod map;

pub struct Lobbies(pub(crate) RwLock<Map<u32, Lobby>>);
pub struct LobbiesId(pub(crate) AtomicU32);
pub struct PlayersId(pub(crate) AtomicU32);

#[get("/lobby/<id>")]
fn lobby(id: u32, state: State<Lobbies>) -> Option<content::Json<String>> {
    let guard = match state.0.read() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let lobby = match guard.get(&id) {
        Some(lobby) => lobby,
        None => return None,
    };

    let json = serde_json::to_string(lobby).unwrap();
    Some(content::Json(json))
}

#[get("/lobbies")]
fn lobbies(state: State<Lobbies>) -> content::Json<String> {
    let guard = match state.0.read() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let json = serde_json::to_string(guard.deref()).unwrap();
    content::Json(json)
}

#[post("/create_lobby/<name>")]
fn create_lobby(
    name: String,
    lobbies_state: State<Lobbies>,
    id_state: State<LobbiesId>,
) -> content::Json<String> {
    let id = id_state.0.fetch_add(1, Ordering::Relaxed);
    let lobby = Lobby::new(id, name, Map::new(), LobbyStatus::Waiting);
    let json = serde_json::to_string(&lobby).unwrap();

    {
        let mut guard = match lobbies_state.0.write() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };
        guard.insert(id, lobby);
    }

    content::Json(json)
}

#[post("/enter_lobby/<lobby_id>/<nickname>")]
fn enter_lobby(
    lobby_id: u32,
    nickname: String,
    _key: ApiKey,
    lobbies_state: State<Lobbies>,
    id_state: State<PlayersId>,
) -> Option<content::Json<String>> {
    {
        let guard = match lobbies_state.0.read() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        let lobby = match guard.get(&lobby_id) {
            Some(lobby) => lobby,
            None => return None,
        };

        if lobby.status() != LobbyStatus::Waiting || lobby.players().len() >= 3 {
            return None;
        }
    }

    let json = {
        let mut guard = match lobbies_state.0.write() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        let lobby = match guard.get_mut(&lobby_id) {
            Some(lobby) if lobby.status() == LobbyStatus::Waiting && lobby.players().len() < 3 => {
                lobby
            }
            _ => return None,
        };

        let id = id_state.0.fetch_add(1, Ordering::Relaxed);
        let player = Player::new(id, nickname, PlayerStatus::NotReady);

        let json = serde_json::to_string(&player).unwrap();
        lobby.players_mut().insert(id, player);

        json
    };

    Some(content::Json(json))
}

#[delete("/leave_lobby/<lobby_id>/<player_id>")]
fn leave_lobby(lobby_id: u32, player_id: u32, _key: ApiKey, state: State<Lobbies>) -> Status {
    {
        let guard = match state.0.read() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        if !guard.contains_key(&lobby_id) {
            return Status::NotFound;
        }
    }

    {
        let mut guard = match state.0.write() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        let lobby = match guard.get_mut(&lobby_id) {
            Some(lobby) => lobby,
            None => return Status::NotFound,
        };

        lobby.players_mut().remove(&player_id);
        if lobby.players().is_empty() {
            guard.remove(&lobby_id);
        }
    }

    Status::Ok
}

#[put("/change_lobby_status/<lobby_id>/<status>")]
fn change_lobby_status(
    lobby_id: u32,
    status: LobbyStatus,
    _key: ApiKey,
    state: State<Lobbies>,
) -> Option<content::Json<String>> {
    {
        let guard = match state.0.read() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        if !guard.contains_key(&lobby_id) {
            return None;
        }
    }

    let json = {
        let mut guard = match state.0.write() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        let lobby = guard.get_mut(&lobby_id)?;
        lobby.set_status(status);

        serde_json::to_string(&lobby).unwrap()
    };

    Some(content::Json(json))
}

#[put("/change_player_status/<lobby_id>/<player_id>/<status>")]
fn change_player_status(
    lobby_id: u32,
    player_id: u32,
    _key: ApiKey,
    status: PlayerStatus,
    state: State<Lobbies>,
) -> Option<content::Json<String>> {
    {
        let guard = match state.0.read() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        if !guard.get(&lobby_id)?.players().contains_key(&player_id) {
            return None;
        }
    }

    let json = {
        let mut guard = match state.0.write() {
            Ok(guard) => guard,
            Err(poison) => poison.into_inner(),
        };

        let lobby = guard.get_mut(&lobby_id)?;
        lobby.players_mut().get_mut(&player_id)?.set_status(status);

        serde_json::to_string(lobby).unwrap()
    };

    Some(content::Json(json))
}

fn main() {
    rocket::ignite()
        .manage(Lobbies {
            0: RwLock::new(Map::new()),
        })
        .manage(PlayersId {
            0: AtomicU32::new(0),
        })
        .manage(LobbiesId {
            0: AtomicU32::new(0),
        })
        .mount(
            "/",
            routes![
                lobby,
                lobbies,
                create_lobby,
                enter_lobby,
                leave_lobby,
                change_lobby_status,
                change_player_status
            ],
        )
        .launch();
}
