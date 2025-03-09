db = db.getSiblingDB("dev-bl-auth");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("EmailAddress")
var address = db.EmailAddress.find();
printjson(address);