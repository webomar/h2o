using CsvHelper.Configuration;
using CsvHelper.Configuration;
using WorkerService.Suppliers.ErpX;
using System.Globalization;

public sealed class ErpXCsvRecordMap : ClassMap<ErpXCsvRecord>
{
    public ErpXCsvRecordMap()
    {
        Map(m => m.RecordType).Name("RecordType");
        Map(m => m.PersNr).Name("PersNr");

        Map(m => m.Crediteur).Name("Crediteur");
        Map(m => m.Rekening).Name("Rekening");
        Map(m => m.OrganisatorischeEenheidCode).Name("OrganisatorischeEenheidCode");

        // =========================
        // Begroting (ALLEEN als RecordType == BEGROTING)
        // =========================
        Map(m => m.BegrotingJaar).Convert(args =>
            args.Row.GetField("RecordType") == "BEGROTING"
                ? args.Row.GetField<int?>("BegrotingJaar")
                : null);

        Map(m => m.KostenplaatsCode).Convert(args =>
            args.Row.GetField("RecordType") == "BEGROTING"
                ? args.Row.GetField("KostenplaatsCode")
                : null);

        Map(m => m.Bedrag).Convert(args =>
            args.Row.GetField("RecordType") == "BEGROTING"
                ? args.Row.GetField<decimal?>("Bedrag")
                : null);

        Map(m => m.Kostensoort).Convert(args =>
            args.Row.GetField("RecordType") == "BEGROTING"
                ? args.Row.GetField("Kostensoort")
                : null);

        // =========================
        // Inhuur (ALLEEN als RecordType == INHUUR)
        // =========================
        Map(m => m.InhuurJaar).Convert(args =>
            args.Row.GetField("RecordType") == "INHUUR"
                ? args.Row.GetField<int?>("InhuurJaar")
                : null);

        Map(m => m.InhuurMaand).Convert(args =>
            args.Row.GetField("RecordType") == "INHUUR"
                ? args.Row.GetField<int?>("InhuurMaand")
                : null);

        Map(m => m.InhuurBedrag).Convert(args =>
            args.Row.GetField("RecordType") == "INHUUR"
                ? args.Row.GetField<decimal?>("InhuurBedrag")
                : null);

        // =========================
        // Transactie (ALLEEN als RecordType == TRANSACTIE)
        // =========================
        Map(m => m.TransactieDatum).Convert(args =>
            args.Row.GetField("RecordType") == "TRANSACTIE"
                ? args.Row.GetField<DateTime?>("TransactieDatum")
                : null);

        Map(m => m.TransactieBedrag).Convert(args =>
            args.Row.GetField("RecordType") == "TRANSACTIE"
                ? args.Row.GetField<decimal?>("TransactieBedrag")
                : null);
    }
}

