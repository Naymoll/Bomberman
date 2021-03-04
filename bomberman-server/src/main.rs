#![feature(proc_macro_hygiene, decl_macro)]

#[macro_use]
extern crate rocket;
extern crate serde;
extern crate serde_json;

mod api_key;
mod id;
mod lobby;
mod player;

use rocket::http::Status;
use rocket::response::content;
use rocket::State;

use api_key::ApiKey;
use id::{LobbiesId, PlayersId};
use lobby::{Lobbies, Lobby, LobbyStatus};
use player::{Player, PlayerStatus};

use std::collections::HashMap;
use std::ops::Deref;
use std::sync::atomic::{AtomicUsize, Ordering};
use std::sync::{Arc, Mutex};

//TODO: Возможно стоит переделать возврат ошибок
// Обсудить старт

#[get("/lobby/<id>")]
fn lobby(id: usize, state: State<Lobbies>) -> Option<content::Json<String>> {
    let guard = match state.0.lock() {
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
    let guard = match state.0.lock() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let lobbies = guard.deref();
    let json = serde_json::to_string(lobbies).unwrap();
    content::Json(json)
}

#[post("/create_lobby/<name>")]
fn create_lobby(
    name: String,
    lobbies_state: State<Lobbies>,
    id_state: State<LobbiesId>,
) -> content::Json<String> {
    let id = id_state.0.fetch_add(1, Ordering::Relaxed);
    let lobby = Lobby::new(id, name, HashMap::new(), LobbyStatus::Waiting);

    let mut guard = match lobbies_state.0.lock() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let json = serde_json::to_string(&lobby).unwrap();
    guard.insert(id, lobby);

    content::Json(json)
}

#[post("/enter_lobby/<lobby_id>/<nickname>")]
fn enter_lobby(
    lobby_id: usize,
    nickname: String,
    key: ApiKey,
    lobbies_state: State<Lobbies>,
    id_state: State<PlayersId>,
) -> Option<content::Json<String>> {
    let mut guard = match lobbies_state.0.lock() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let lobby = match guard.get_mut(&lobby_id) {
        Some(lobby) if lobby.status() == LobbyStatus::Waiting => lobby,
        _ => return None,
    };

    let id = id_state.0.fetch_add(1, Ordering::Relaxed);
    let player = Player::new(id, nickname, PlayerStatus::NotReady);

    let json = serde_json::to_string(&player).unwrap();
    lobby.players_mut().insert(id, player);

    Some(content::Json(json))
}

#[post("/leave_lobby/<lobby_id>/<player_id>")]
fn leave_lobby(lobby_id: usize, player_id: usize, key: ApiKey, state: State<Lobbies>) -> Status {
    let mut guard = match state.0.lock() {
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

    Status::Ok
}

#[put("/change_lobby_status/<lobby_id>/<status>")]
fn change_lobby_status(
    lobby_id: usize,
    status: LobbyStatus,
    key: ApiKey,
    state: State<Lobbies>,
) -> Option<content::Json<String>> {
    let mut guard = match state.0.lock() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let lobby = match guard.get_mut(&lobby_id) {
        Some(lobby) => lobby,
        None => return None,
    };

    lobby.set_status(status);

    let json = serde_json::to_string(lobby).unwrap();
    Some(content::Json(json))
}

#[put("/change_player_status/<lobby_id>/<player_id>/<status>")]
fn change_player_status(
    lobby_id: usize,
    player_id: usize,
    key: ApiKey,
    status: PlayerStatus,
    state: State<Lobbies>,
) -> Option<content::Json<String>> {
    let mut guard = match state.0.lock() {
        Ok(guard) => guard,
        Err(poison) => poison.into_inner(),
    };

    let lobby = match guard.get_mut(&lobby_id) {
        Some(lobby) => lobby,
        None => return None,
    };

    let player = match lobby.players_mut().get_mut(&player_id) {
        Some(player) => player,
        None => return None,
    };

    player.set_status(status);

    let json = serde_json::to_string(lobby).unwrap();
    Some(content::Json(json))
}

fn main() {
    rocket::ignite()
        .manage(Lobbies {
            0: Arc::new(Mutex::new(HashMap::new())),
        })
        .manage(PlayersId {
            0: AtomicUsize::new(0),
        })
        .manage(LobbiesId {
            0: AtomicUsize::new(0),
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
