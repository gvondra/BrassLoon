db = db.getSiblingDB("dev-bl-account");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("Domain")
var domain = db.Domain.find();
printjson(domain);
