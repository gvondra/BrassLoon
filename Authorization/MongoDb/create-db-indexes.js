function CreateIndex(collection, keys, options) {
    let allIndexes = collection.getIndexes();
    var create = true;
    for (var index of allIndexes) {    
        if (index["name"] == options["name"]) {
            create = false;
        }
    }
    if (create) {
        print(`Create index ${options["name"]}`);
        collection.createIndex(keys, options);
    }
}
db = db.getSiblingDB("dev-bl-auth");
// Client
CreateIndex(db.Client, { "DomainId": 1 }, { "name": "IX_Client_DomainId" });
// EmailAddress
CreateIndex(db.EmailAddress, { "AddressHash": 1 }, { "name": "IX_EmailAddress_AddressHash", "unique": true });
// Role
CreateIndex(db.Role, { "DomainId": 1, "PolicyName": 1 }, { "name": "IX_Role_DomainId_PolicyName", "unique": true });
// SigningKey
CreateIndex(db.SigningKey, { "DomainId": 1 }, { "name": "IX_SigningKey_DomainId" });
// User
CreateIndex(db.User, { "DomainId": 1, "ReferenceId": 1 }, { "name": "IX_User_DomainIdReferenceId", "unique": true });
CreateIndex(db.User, { "DomainId": 1, "EmailAddressId": 1 }, { "name": "IX_User_DomainIdEmailAddressId", "unique": true });