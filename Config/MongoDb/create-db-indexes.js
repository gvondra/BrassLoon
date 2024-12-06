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
// Item
CreateIndex(db.Item, { "DomainId": 1, "Code": 1 }, { "name": "IX_Item_DomainId_Code", "unique": true });
CreateIndex(db.ItemHistory, { "ItemId": 1 }, { "name": "IX_ItemHistory_LookupId" });
CreateIndex(db.ItemHistory, { "DomainId": 1 }, { "name": "IX_ItemHistory_DomainId" });
// Lookup
CreateIndex(db.Lookup, { "DomainId": 1, "Code": 1 }, { "name": "IX_Lookup_DomainId_Code", "unique": true });
CreateIndex(db.LookupHistory, { "LookupId": 1 }, { "name": "IX_LookupHistory_LookupId" });
CreateIndex(db.LookupHistory, { "DomainId": 1 }, { "name": "IX_LookupHistory_DomainId" });
