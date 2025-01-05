db = db.getSiblingDB("dev-bl-config");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("item")
var item = db.Item.find({ "DomainId": UUID("5ec76c1b-3dcc-4961-b076-9cfb53acb38b") });
printjson(item);
print("item history")
var itemHistories = db.ItemHistory.find({ "DomainId": UUID("5ec76c1b-3dcc-4961-b076-9cfb53acb38b") });
printjson(itemHistories);