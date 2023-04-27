using System.ComponentModel.Composition;
using System.Data.Entity.Core.Metadata.Edm;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

//...
//Datetime CreatedAt { CreationTime; DenyUserEdit; }
//Logging { AllProperties; }
//
//...

namespace RhetosExtensions
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("MonitoredRecord")]
    public class MonitoredRecordInfo : IConceptInfo
    {
        //EntityInfo Entity oznacava na kojem se entitetu nalazi naš custom concept
        //U MonitoredRecordMacro klasi se vidi kako taj entity koristimo kako bi dodavali
        //nove koncepte na njega
        [ConceptKey]
        public EntityInfo Entity { get; set; }
    }

    [Export(typeof(IConceptMacro))]
    public class MonitoredRecordMacro : IConceptMacro<MonitoredRecordInfo>
    {
        public IEnumerable<IConceptInfo> CreateNewConcepts(MonitoredRecordInfo conceptInfo, IDslModel existingConcepts)
        {
            var newConcepts = new List<IConceptInfo>();

            //prvo stvori polja (koncepte bez parametara)
            //DateTime CreatedAt...
            var createdAt = new DateTimePropertyInfo { DataStructure = conceptInfo.Entity, Name = "CreatedAt" };
            newConcepts.Add(createdAt);

            //Logging...
            var logging = new EntityLoggingInfo { Entity = conceptInfo.Entity };
            newConcepts.Add(logging);

            //dodavanje parametara (properties) na koncepte
            //DateTime CreatedAt { CreationTime; DenyUserEdit; }                        //...CreatedAt{
            newConcepts.Add(new CreationTimeInfo { Property = createdAt });             //      CreationTime;
            newConcepts.Add(new DenyUserEditPropertyInfo { Property = createdAt });     //      DenyUserEdit;}

                                                                                        //...Logging{
            newConcepts.Add(new AllPropertiesLoggingInfo { EntityLogging = logging });  //      AllProperties; }

            return newConcepts;
        }
    }
}