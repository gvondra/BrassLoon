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
db = db.getSiblingDB("dev-bl-account");
// // AccountUser
// CreateIndex(db.AccountUser, { "AccountGuid": 1, "UserGuid": 1 }, { "name": "IX_AccountUser_AccountGuid_UserGuid", "unique": true  });
// CreateIndex(db.AccountUser, { "AccountGuid": 1 }, { "name": "IX_AccountUser_AccountGuid"  });
// CreateIndex(db.AccountUser, { "UserGuid": 1 }, { "name": "IX_AccountUser_UserGuid"  });
// // Client
// CreateIndex(db.Client, { "AccountGuid": 1 }, { "name": "IX_Client_AccountGuid" });
// // ClientCredential
// CreateIndex(db.ClientCredential, { "ClientId": 1 }, { "name": "IX_ClientCredential_ClientId" });
// // Domain
// CreateIndex(db.Domain, { "AccountGuid": 1 }, { "name": "IX_Domain_AccountGuid" });
// // EmailAddress
// CreateIndex(db.EmailAddress, { "Address": 1 }, { "name": "IX_EmailAddress_Address", "unique": true });
// // User
// CreateIndex(db.User, { "ReferenceId": 1 }, { "name": "IX_User_ReferenceId", "unique": true });
// CreateIndex(db.User, { "EmailAddressGuid": 1 }, { "name": "IX_User_EmailAddressGuid" });
// // UserInvitation
// CreateIndex(db.UserInvitation, { "AccountGuid": 1 }, { "name": "IX_UserInvitation_AccountGuid" });