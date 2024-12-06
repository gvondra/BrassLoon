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
db = db.getSiblingDB("dev-bl-config");
CreateIndex(db.Lookup, { "DomainId": 1, "Code": 1 }, { "name": "IX_Lookup_DomainId_Code", "unique": true });
CreateIndex(db.LookupHistory, { "LookupId": 1 }, { "name": "IX_LookupHistory_LookupId" });
CreateIndex(db.LookupHistory, { "DomainId": 1 }, { "name": "IX_LookupHistory_DomainId" });
