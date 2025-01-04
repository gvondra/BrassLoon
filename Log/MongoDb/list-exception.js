db = db.getSiblingDB("dev-bl-log");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("exception")
var exception = db.Exception.find({ "CreateTimestamp": { $gt: ISODate('2025-01-04T15:50:53.905Z') } });
printjson(exception);