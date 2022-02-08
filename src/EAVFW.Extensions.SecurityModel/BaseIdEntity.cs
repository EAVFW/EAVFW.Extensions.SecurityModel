using DotNetDevOps.Extensions.EAVFramework;
using DotNetDevOps.Extensions.EAVFramework.Shared;
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
    public class BaseIdEntity<TIdentity> : DynamicEntity where TIdentity : IdentityBase
    {

        [DataMember(Name = "id")]
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [DataMember(Name = "modifiedbyid")]
        [JsonProperty("modifiedbyid")]
        [JsonPropertyName("modifiedbyid")]
        public Guid? ModifiedById { get; set; }

        [ForeignKey("ModifiedById")]
        [JsonProperty("modifiedby")]
        [JsonPropertyName("modifiedby")]
        [DataMember(Name = "modifiedby")]
        public TIdentity ModifiedBy { get; set; }

        [DataMember(Name = "createdbyid")]
        [JsonProperty("createdbyid")]
        [JsonPropertyName("createdbyid")]
        public Guid? CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        [JsonProperty("createdby")]
        [JsonPropertyName("createdby")]
        [DataMember(Name = "createdby")]
        public TIdentity CreatedBy { get; set; }

        [DataMember(Name = "modifiedon")]
        [JsonProperty("modifiedon")]
        [JsonPropertyName("modifiedon")]
        public DateTime? ModifiedOn { get; set; }

        [DataMember(Name = "createdon")]
        [JsonProperty("createdon")]
        [JsonPropertyName("createdon")]
        public DateTime? CreatedOn { get; set; }

        [DataMember(Name = "rowversion")]
        [JsonProperty("rowversion")]
        [JsonPropertyName("rowversion")]
        public byte[] RowVersion { get; set; }

    }
}