db = db.getSiblingDB("dev-bl-address");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("email address")
var email = db.EmailAddress.find();
printjson(email);