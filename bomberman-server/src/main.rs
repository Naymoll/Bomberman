#![feature(proc_macro_hygiene, decl_macro)]

#[macro_use]
extern crate rocket;
extern crate serde;
extern crate serde_json;

use std::collections::HashSet;
use std::sync::atomic::{AtomicUsize, Ordering};
use std::sync::Mutex;

use rocket::response;
use serde::{Deserialize, Serialize};

//TODO: UNSAFE - Боль
// ПЕРЕДЕЛАТЬ НАХУЙ
pub unsafe fn last_player_id() -> usize {
    static mut LAST_PLAYER_ID: AtomicUsize = AtomicUsize::new(0);
    LAST_PLAYER_ID.fetch_add(1, Ordering::Relaxed)
}

pub unsafe fn last_lobby_id() -> usize {
    static mut LAST_LOBBY_ID: AtomicUsize = AtomicUsize::new(0);
    LAST_LOBBY_ID.fetch_add(1, Ordering::Relaxed)
}

pub unsafe fn get_lobbies() -> &'static mut Vec<Lobby> {
    static mut LOBBIES: Vec<Lobby> = Vec::new();
    &mut LOBBIES
}

pub unsafe fn add_lobby(lobby: Lobby) {
    let _ = Mutex::new(0).lock().unwrap();

    let lobbies = get_lobbies();
    lobbies.push(lobby);
}

pub unsafe fn remove_lobby(lobby: Lobby) {
    let _ = Mutex::new(0).lock().unwrap();

    let lobbies = get_lobbies();
    let pos = lobbies.iter().position(|l| l.id == lobby.id).unwrap();
    lobbies.remove(pos);
}

#[derive(Deserialize, Serialize, Hash, Eq, PartialEq)]
pub struct Player {
    id: usize,
    nickname: String,
}

impl Player {
    pub fn new(nickname: String) -> Self {
        unsafe {
            Player {
                id: last_player_id(),
                nickname,
            }
        }
    }

    pub fn id(&self) -> usize {
        self.id
    }

    pub fn nickname(&self) -> String {
        self.nickname.clone()
    }

    pub fn set_nickname(&mut self, nickname: String) {
        self.nickname = nickname;
    }
}

#[derive(Deserialize, Serialize)]
pub struct Lobby {
    id: usize,
    players: HashSet<Player>,
}

impl Lobby {
    pub fn new() -> Self {
        unsafe {
            Lobby {
                id: last_lobby_id(),
                players: HashSet::new(),
            }
        }
    }

    pub fn id(&self) -> usize {
        self.id
    }

    pub fn add_player(&mut self, player: Player) {
        self.players.insert(player);
    }

    pub fn remove_player(&mut self, player: Player) -> bool {
        self.players.remove(&player)
    }
}

#[get("/lobbies")]
fn lobbies() -> response::content::Json<String> {
    unsafe {
        let lobbies = get_lobbies();
        let json = serde_json::to_string(lobbies).unwrap();
        response::content::Json(json)
    }
}

#[post("/create_lobby")]
fn create_lobby() -> response::content::Json<String> {
    let lobby = Lobby::new();
    let json = serde_json::to_string(&lobby).unwrap();
    unsafe { add_lobby(lobby) }
    response::content::Json(json)
}

#[post("/new_player/<nickname>")]
fn new_player(nickname: String) -> response::content::Json<String> {
    let player = Player::new(nickname);
    let json = serde_json::to_string(&player).unwrap();
    response::content::Json(json)
}

fn main() {
    rocket::ignite()
        .mount("/", routes![lobbies, create_lobby, new_player])
        .launch();
}
