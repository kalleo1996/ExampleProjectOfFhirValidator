
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using Hl7.Fhir.Validation;
using System.Reflection;



namespace ExampleProjectOfFhirValidator
{
    class Program
    {


        static void Main(string[] args)
        {

            string text = File.ReadAllText(@"./ObservationSucess.json");
            // var obs = JsonSerializer.Deserialize<Observation>(text);
            //JObject json = JObject.Parse(text);

            var fhirJsonParser = new FhirJsonParser();


            var responseResource = fhirJsonParser.Parse<Resource>(text);


            ValidateResources(responseResource);



        }

        private static void ValidateResources(Resource obs)
        {


            //without any resources  
            //var validator = new Validator();
            //var outcome = validator.Validate(obs);
            //Console.WriteLine($"{outcome}");

            //with fhir specification as resource 
            //fhir r4 specification
            //Todo : will use , once the issue related to this method is fixed 
            // var FHRR4Specificationresolver = ZipSource.CreateValidationSource();
            //Getting Executable Path
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fhirSpecificationPath = Path.Combine(currentDirectory, "Profiles\\specification");
            var FHRR4Specificationresolver = new DirectorySource(fhirSpecificationPath);

            //directory resource for custom profiles 
            string pathToCustomDir = Path.Combine(currentDirectory, "Profiles\\DocumentReference");
            var customResolver = new DirectorySource(pathToCustomDir);

            //directory resource for valuesets
            string PathToValueSetDir = Path.Combine(currentDirectory, "Profiles\\ValueSets");
            var valueSetResolver = new DirectorySource(PathToValueSetDir);

            //directory resource for nobasis resources 
            string PathNoBaisisResolverDir = Path.Combine(currentDirectory, "Profiles\\no-basis-profiles");
            var noBasisResolver = new DirectorySource(PathNoBaisisResolverDir);

            //adding the settings to Validator
            var settings = ValidationSettings.CreateDefault();
            settings.ResourceResolver = new CachedResolver(new MultiResolver(FHRR4Specificationresolver, noBasisResolver, customResolver));
            //standardized terminologies, including code systems, value sets, concept maps
            settings.TerminologyService = new LocalTerminologyService(new MultiResolver(FHRR4Specificationresolver, noBasisResolver, valueSetResolver));

            //creating the Validator instance 
            settings.ResourceResolver = new CachedResolver(new MultiResolver(FHRR4Specificationresolver, noBasisResolver, customResolver));
            var validator = new Validator(settings);
            // var outcome = validator.Validate(obs);
            var outcome = validator.Validate(obs, new[] { "http://dips.no/fhir/R4/StructureDefinition/DIPSR4DocumentReference" });
            Console.WriteLine($"{outcome}");

        }

    }
}