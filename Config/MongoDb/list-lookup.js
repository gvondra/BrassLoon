db = db.getSiblingDB("dev-bl-config");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("lookup")
var lookup = db.Lookup.find();
printjson(lookup);
print("lookup history")
var lookupHistories = db.LookupHistory.find();
printjson(lookupHistories);