db = db.getSiblingDB("dev-bl-config");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("item")
var item = db.Item.find();
printjson(item);
print("item history")
var itemHistories = db.ItemHistory.find();
printjson(itemHistories);