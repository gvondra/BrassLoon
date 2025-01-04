db = db.getSiblingDB("dev-bl-log");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("exception")
var exception = db.Exception.find();
printjson(exception);