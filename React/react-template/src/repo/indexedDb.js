import { openDB } from "idb";

export const initDb = async() => {
    return openDB('myDb',1, {
        upgrade(db){
            if (!db.objectStoreNames.contains('user')) {
                db.createObjectStore('user', {
                    keyPath: 'id',
                    autoIncrement: true,
                });
            }
        },
    });
}
