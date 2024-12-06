db = db.getSiblingDB("dev-bl-config");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("item")
var item = db.Item.getIndexes();
printjson(item);
print("item history")
var itemHistories = db.ItemHistory.getIndexes();
printjson(itemHistories);