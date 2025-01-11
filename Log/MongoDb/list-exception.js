db = db.getSiblingDB("dev-bl-log");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("exception")
var exception = db.Exception.find({ "CreateTimestamp": { $gt: ISODate('2025-01-11T14:00:00.000Z') } });
printjson(exception);