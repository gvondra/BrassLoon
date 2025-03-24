db = db.getSiblingDB("dev-bl-work-task");

db.WorkTask.updateOne(
    { _id: UUID('9fe851c3-b1b8-4157-ac29-d3b88593ab9f') },
    { $set: { AssignedToUserId: "" } }
);