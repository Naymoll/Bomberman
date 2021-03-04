use rocket::http::Status;
use rocket::request::{FromRequest, Outcome};
use rocket::Request;

pub struct ApiKey(String);

//TODO: Переделать ключ
impl ApiKey {
    pub fn is_valid_key(key: &str) -> bool {
        key == "123"
    }
}

impl<'a, 'r> FromRequest<'a, 'r> for ApiKey {
    type Error = ();

    fn from_request(request: &'a Request<'r>) -> Outcome<Self, Self::Error> {
        let keys: Vec<_> = request.headers().get("api-key").collect();
        match keys.len() {
            1 if ApiKey::is_valid_key(keys[0]) => Outcome::Success(ApiKey(keys[0].to_string())),
            _ => Outcome::Failure((Status::Forbidden, ())),
        }
    }
}
