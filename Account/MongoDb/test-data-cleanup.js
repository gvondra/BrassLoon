db = db.getSiblingDB("dev-bl-account");
var accounts = db.Account.find({ Name: { $regex: /^test-generated-/, $options: 'i' }});
for (let account of accounts) {
    print(`Deleting account ${account.Name}`);

    print('Delete domains');
    db.Domain.deleteMany({ AccountGuid: { $eq: account._id} });
    
    print('Delete clients');
    db.Client.deleteMany({ AccountId: { $eq: account._id} });

    print('Delete account user');
    db.AccountUser.deleteMany({ AccountGuid: { $eq: account._id} });

    print('Delete account');
    db.Account.deleteOne({ _id: { $eq: account._id} });
}