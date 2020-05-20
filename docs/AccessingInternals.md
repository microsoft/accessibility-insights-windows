## Accessing Internals

You can safely ignore this entire file if you make everything publicly available from your assembly. If you wish to avoid having your internal interfaces and classes public for testing, keep reading…

.NET provides a way to expose internal classes and data, using the InternalsVisibleTo attribute. This mechanism gets a little bit tricky when StrongName assemblies are involved. Since the same file will be used for both signed and unsigned build loops, we need to be flexible enough to support both sets of requirements in a single file. Here are the steps:

### Add the InternalsVisibleTo attribute
Add the following block to the end of your AssemblyInfo.cs file, replacing <YourAssemblyName> with the file name of your assembly--note that the ENABLE_SIGNING flag is already set as part of the build and that the 3 keys are ***intentionally different***:
```
#if ENABLE_SIGNING
[assembly: InternalsVisibleTo("<YourAssemblyName>Tests,PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2,PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]    
#else
[assembly: InternalsVisibleTo("<YourAssemblyName>Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
```
The PublicKey values for DynamicProxyGenAssembly2 are fixed and ***must*** remain unchanged. The PublicKey for the main assembly must remain in sync with the public key used for the entire project. You can safely omit the DynamicProxyGenAssembly2 entry if you don't unit test with Moq.

### Validation of signing
Signing can only be fully validated in the signed build environment, which is not externally accessible. If you are making any changes that change the InternalsVisibleTo in any way, please create the PR as usual, then create an issue to request validation of the signing changes.
