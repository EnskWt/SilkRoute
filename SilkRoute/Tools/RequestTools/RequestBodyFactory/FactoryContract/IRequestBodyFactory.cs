namespace SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract
{
    internal interface IRequestBodyFactory
    {
        int Priority { get; }
        bool CanCreate(object val);
        HttpContent Create(object val);
    }
}
