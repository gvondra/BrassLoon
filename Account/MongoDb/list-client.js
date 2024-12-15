db = db.getSiblingDB("dev-bl-account");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("Client")
var client = db.Client.find();
printjson(client);
