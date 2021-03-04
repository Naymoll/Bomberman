use std::sync::atomic::AtomicUsize;

pub struct PlayersId(pub(crate) AtomicUsize);
pub struct LobbiesId(pub(crate) AtomicUsize);
