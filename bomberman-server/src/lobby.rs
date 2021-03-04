use crate::player::Player;

use rocket::http::RawStr;
use rocket::request::FromParam;
use std::collections::HashMap;
use std::hash::{Hash, Hasher};
use std::sync::{Arc, Mutex};

pub struct Lobbies(pub(crate) Arc<Mutex<HashMap<usize, Lobby>>>);

#[derive(serde::Serialize, Copy, Clone, Eq, PartialEq)]
pub enum LobbyStatus {
    Waiting,
    InGame,
}

impl<'a> FromParam<'a> for LobbyStatus {
    type Error = ();

    fn from_param(param: &'a RawStr) -> Result<Self, Self::Error> {
        match param.url_decode_lossy().as_str() {
            "0" => Ok(LobbyStatus::Waiting),
            "1" => Ok(LobbyStatus::InGame),
            _ => Err(()),
        }
    }
}

//ID unique for each Lobby instance

#[derive(serde::Serialize, Clone)]
pub struct Lobby {
    id: usize,
    name: String,
    players: HashMap<usize, Player>,
    status: LobbyStatus,
}

impl Lobby {
    pub fn new(
        id: usize,
        name: String,
        players: HashMap<usize, Player>,
        status: LobbyStatus,
    ) -> Self {
        Lobby {
            id,
            name,
            players,
            status,
        }
    }

    pub fn id(&self) -> usize {
        self.id
    }

    pub fn players(&self) -> &HashMap<usize, Player> {
        &self.players
    }

    pub fn players_mut(&mut self) -> &mut HashMap<usize, Player> {
        &mut self.players
    }

    pub fn status(&self) -> LobbyStatus {
        self.status
    }

    pub fn set_status(&mut self, status: LobbyStatus) {
        self.status = status;
    }
}

impl Hash for Lobby {
    fn hash<H: Hasher>(&self, state: &mut H) {
        self.id.hash(state);
    }
}

impl PartialEq for Lobby {
    fn eq(&self, other: &Self) -> bool {
        self.id == other.id
    }
}

impl Eq for Lobby {}
