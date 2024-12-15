db = db.getSiblingDB("dev-bl-account");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("Account")
var account = db.Account.find();
printjson(account);
print("AccountUser")
var accountUser = db.AccountUser.find();
printjson(accountUser);