db = db.getSiblingDB("dev-bl-account");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("EmailAddress")
var emailAddresses = db.EmailAddress.find();
printjson(emailAddresses);