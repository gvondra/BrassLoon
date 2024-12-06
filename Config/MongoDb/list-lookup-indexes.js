db = db.getSiblingDB("dev-bl-config");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("lookup")
var lookup = db.Lookup.getIndexes();
printjson(lookup);
print("lookup history")
var lookupHistories = db.LookupHistory.getIndexes();
printjson(lookupHistories);