namespace DiplomApp.Accounts
{
    interface IAccount
    {
        int ID { get; set; }

        string Login { get; set; }

        byte[] Salt { get; set; }

        byte[] Key { get; set; }
    }
}
