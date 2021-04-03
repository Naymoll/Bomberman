use crate::map::Map;
use rocket::http::RawStr;
use rocket::request::FromParam;
use serde::Serialize;
use std::collections::HashMap;

#[derive(Serialize, Copy, Clone, Eq, PartialEq)]
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

#[derive(Serialize)]
pub struct Lobby {
    id: u32,
    name: String,
    players: Map<u32, Player>,
    status: LobbyStatus,
}

impl Lobby {
    pub fn new(id: u32, name: String, players: Map<u32, Player>, status: LobbyStatus) -> Self {
        Lobby {
            id,
            name,
            players,
            status,
        }
    }

    pub fn id(&self) -> u32 {
        self.id
    }

    pub fn players(&self) -> &HashMap<u32, Player> {
        &self.players
    }

    pub fn players_mut(&mut self) -> &mut HashMap<u32, Player> {
        &mut self.players
    }

    pub fn status(&self) -> LobbyStatus {
        self.status
    }

    pub fn set_status(&mut self, status: LobbyStatus) {
        self.status = status;
    }
}

#[derive(Serialize, Clone, Copy, Eq, PartialEq)]
pub enum PlayerStatus {
    NotReady,
    Ready,
}

impl<'a> FromParam<'a> for PlayerStatus {
    type Error = ();

    fn from_param(param: &'a RawStr) -> Result<Self, Self::Error> {
        match param.url_decode_lossy().as_str() {
            "0" => Ok(PlayerStatus::NotReady),
            "1" => Ok(PlayerStatus::Ready),
            _ => Err(()),
        }
    }
}

#[derive(Serialize)]
pub struct Player {
    id: u32,
    nickname: String,
    status: PlayerStatus,
}

impl Player {
    pub fn new(id: u32, nickname: String, status: PlayerStatus) -> Self {
        Player {
            id,
            nickname,
            status,
        }
    }

    pub fn id(&self) -> u32 {
        self.id
    }

    pub fn status(&self) -> PlayerStatus {
        self.status
    }

    pub fn set_status(&mut self, status: PlayerStatus) {
        self.status = status;
    }
}
