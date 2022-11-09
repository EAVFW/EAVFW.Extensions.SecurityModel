using EAVFramework;
using EAVFramework.Shared;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EAVFW.Extensions.SecurityModel
{

    [BaseEntity]
    [Serializable]
    [GenericTypeArgument(ArgumentName = "TIdentity", ManifestKey ="Identity")]
    public class BaseIdEntity<TIdentity> : DynamicEntity where TIdentity : DynamicEntity
    {

        [DataMember(Name = "id")]
        [EntityField(AttributeKey ="Id")]
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [DataMember(Name = "modifiedbyid")]
        [EntityField(AttributeKey = "Modified By")]
        [JsonProperty("modifiedbyid")]
        [JsonPropertyName("modifiedbyid")]
        public Guid? ModifiedById { get; set; }

        [ForeignKey("ModifiedById")]
        [JsonProperty("modifiedby")]
        [JsonPropertyName("modifiedby")]
        [DataMember(Name = "modifiedby")]
        public TIdentity ModifiedBy { get; set; }

        [DataMember(Name = "createdbyid")]
        [EntityField(AttributeKey = "Created By")]
        [JsonProperty("createdbyid")]
        [JsonPropertyName("createdbyid")]
        public Guid? CreatedById { get; set; }

        [ForeignKey("CreatedById")]
      
        [JsonProperty("createdby")]
        [JsonPropertyName("createdby")]
        [DataMember(Name = "createdby")]
        public TIdentity CreatedBy { get; set; }

        [DataMember(Name = "modifiedon")]
        [EntityField(AttributeKey = "Modified On")]
        [JsonProperty("modifiedon")]
        [JsonPropertyName("modifiedon")]
        public DateTime? ModifiedOn { get; set; }

        [DataMember(Name = "createdon")]
        [EntityField(AttributeKey = "Created On")]
        [JsonProperty("createdon")]
        [JsonPropertyName("createdon")]
        public DateTime? CreatedOn { get; set; }

        [DataMember(Name = "rowversion")]
        [EntityField(AttributeKey = "Row Version")]
        [JsonProperty("rowversion")]
        [JsonPropertyName("rowversion")]
        public byte[] RowVersion { get; set; }

    }
}