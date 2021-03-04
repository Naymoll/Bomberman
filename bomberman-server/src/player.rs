use rocket::http::RawStr;
use rocket::request::FromParam;
use std::hash::{Hash, Hasher};

//ID unique for each Player instance

#[derive(serde::Serialize, Copy, Clone, Eq, PartialEq)]
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

#[derive(serde::Serialize, Clone)]
pub struct Player {
    id: usize,
    nickname: String,
    status: PlayerStatus,
}

impl Player {
    pub fn new(id: usize, nickname: String, status: PlayerStatus) -> Self {
        Player {
            id,
            nickname,
            status,
        }
    }

    pub fn id(&self) -> usize {
        self.id
    }

    pub fn status(&self) -> PlayerStatus {
        self.status
    }

    pub fn set_status(&mut self, status: PlayerStatus) {
        self.status = status;
    }
}

impl Hash for Player {
    fn hash<H: Hasher>(&self, state: &mut H) {
        self.id.hash(state);
    }
}

impl PartialEq for Player {
    fn eq(&self, other: &Self) -> bool {
        self.id == other.id
    }
}

impl Eq for Player {}
