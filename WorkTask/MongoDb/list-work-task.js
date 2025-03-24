db = db.getSiblingDB("dev-bl-work-task");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("WorkTask")
var task = db.WorkTask.find({ UpdateTimestamp: { $gt: ISODate('2025-03-24') } }).toArray();
printjson(task);

// print("WorkTaskComment")
// var comment = db.WorkTaskComment.find().toArray();
// printjson(comment);

// print("WorkTaskContext")
// var context = db.WorkTaskContext.find().toArray();
// printjson(context);