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
db = db.getSiblingDB("dev-bl-address");
// Address
CreateIndex(db.Address, { "DomainId": 1 }, { "name": "IX_Address_DomainId" });
CreateIndex(db.Address, { "DomainId": 1, "Hash": 1 }, { "name": "IX_Address_Hash" });
// EmailAddress
CreateIndex(db.EmailAddress, { "DomainId": 1 }, { "name": "IX_EmailAddress_DomainId" });
CreateIndex(db.EmailAddress, { "DomainId": 1, "Hash": 1 }, { "name": "IX_EmailAddress_Hash" });
// Phone
CreateIndex(db.Phone, { "DomainId": 1 }, { "name": "IX_Phone_DomainId" });
CreateIndex(db.Phone, { "DomainId": 1, "Hash": 1 }, { "name": "IX_Phone_Hash" });