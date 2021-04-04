use serde::ser::SerializeSeq;
use serde::{Serialize, Serializer};
use std::collections::HashMap;
use std::ops::{Deref, DerefMut};

pub struct Map<K, V>(HashMap<K, V>);

impl<K, V> Map<K, V> {
    pub fn new() -> Self {
        Map { 0: HashMap::new() }
    }
}

impl<K, V> Deref for Map<K, V> {
    type Target = HashMap<K, V>;

    fn deref(&self) -> &Self::Target {
        &self.0
    }
}

impl<K, V> DerefMut for Map<K, V> {
    fn deref_mut(&mut self) -> &mut Self::Target {
        &mut self.0
    }
}

impl<K, V> Serialize for Map<K, V>
where
    V: Serialize,
{
    fn serialize<S>(&self, serializer: S) -> Result<<S as Serializer>::Ok, <S as Serializer>::Error>
    where
        S: Serializer,
    {
        let mut seq = serializer.serialize_seq(Some(self.0.len()))?;
        for (_, value) in &self.0 {
            seq.serialize_element(value)?;
        }
        seq.end()
    }
}
