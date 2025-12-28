namespace WorkerService.Mapping
{
    using WorkerService.Domain;

    public interface ISupplierMapper<TSource>
    {
        /// <summary>
        /// Unieke sleutel van de leverancier (bijv. "Youforce", "ErpX")
        /// </summary>
        string SupplierKey { get; }

        /// <summary>
        /// Map leverancier-specifieke data naar het uniforme domeinmodel
        /// </summary>
        Employee Map(TSource source);
    }
}
