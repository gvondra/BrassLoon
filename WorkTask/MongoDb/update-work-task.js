db = db.getSiblingDB("dev-bl-work-task");

db.WorkTask.updateOne(
    { _id: UUID('bb14a6a8-4371-450f-be1d-24316e838b6e') },
    { $set: { AssignedToUserId: "" } }
);