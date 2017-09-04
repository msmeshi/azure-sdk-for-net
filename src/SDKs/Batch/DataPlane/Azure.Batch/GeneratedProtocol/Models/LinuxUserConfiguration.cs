// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.Azure.Batch.Protocol.Models
{
    using System.Linq;

    /// <summary>
    /// Properties used to create a user account on a Linux node.
    /// </summary>
    public partial class LinuxUserConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the LinuxUserConfiguration class.
        /// </summary>
        public LinuxUserConfiguration() { }

        /// <summary>
        /// Initializes a new instance of the LinuxUserConfiguration class.
        /// </summary>
        /// <param name="uid">The user ID of the user account.</param>
        /// <param name="gid">The group ID for the user account.</param>
        /// <param name="sshPrivateKey">The SSH private key for the user
        /// account.</param>
        public LinuxUserConfiguration(int? uid = default(int?), int? gid = default(int?), string sshPrivateKey = default(string))
        {
            this.Uid = uid;
            this.Gid = gid;
            this.SshPrivateKey = sshPrivateKey;
        }

        /// <summary>
        /// Gets or sets the user ID of the user account.
        /// </summary>
        /// <remarks>
        /// The uid and gid properties must be specified together or not at
        /// all. If not specified the underlying operating system picks the
        /// uid.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uid")]
        public int? Uid { get; set; }

        /// <summary>
        /// Gets or sets the group ID for the user account.
        /// </summary>
        /// <remarks>
        /// The uid and gid properties must be specified together or not at
        /// all. If not specified the underlying operating system picks the
        /// gid.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(PropertyName = "gid")]
        public int? Gid { get; set; }

        /// <summary>
        /// Gets or sets the SSH private key for the user account.
        /// </summary>
        /// <remarks>
        /// The private key must not be password protected. The private key is
        /// used to automatically configure asymmetric-key based authentication
        /// for SSH between nodes in a Linux pool when the pool's
        /// enableInterNodeCommunication property is true (it is ignored if
        /// enableInterNodeCommunication is false). It does this by placing the
        /// key pair into the user's .ssh directory. If not specified,
        /// password-less SSH is not configured between nodes (no modification
        /// of the user's .ssh directory is done).
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(PropertyName = "sshPrivateKey")]
        public string SshPrivateKey { get; set; }

    }
}