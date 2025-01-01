db = db.getSiblingDB("dev-bl-log");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("event id")
var eventId = db.EventId.find();
printjson(eventId);