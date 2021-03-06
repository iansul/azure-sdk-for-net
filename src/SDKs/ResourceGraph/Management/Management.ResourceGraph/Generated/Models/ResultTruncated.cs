// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.ResourceGraph.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for ResultTruncated.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResultTruncated
    {
        [EnumMember(Value = "true")]
        True,
        [EnumMember(Value = "false")]
        False
    }
    internal static class ResultTruncatedEnumExtension
    {
        internal static string ToSerializedValue(this ResultTruncated? value)
        {
            return value == null ? null : ((ResultTruncated)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this ResultTruncated value)
        {
            switch( value )
            {
                case ResultTruncated.True:
                    return "true";
                case ResultTruncated.False:
                    return "false";
            }
            return null;
        }

        internal static ResultTruncated? ParseResultTruncated(this string value)
        {
            switch( value )
            {
                case "true":
                    return ResultTruncated.True;
                case "false":
                    return ResultTruncated.False;
            }
            return null;
        }
    }
}
