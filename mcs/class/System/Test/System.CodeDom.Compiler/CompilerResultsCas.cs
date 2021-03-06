//
// CompilerResultsCas.cs 
//	- CAS unit tests for System.CodeDom.Compiler.CompilerResults
//
// Author:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace MonoCasTests.System.CodeDom.Compiler {

	[TestFixture]
	[Category ("CAS")]
	public class CompilerResultsCas {

		[SetUp]
		public void SetUp ()
		{
			if (!SecurityManager.SecurityEnabled)
				Assert.Ignore ("SecurityManager.SecurityEnabled is OFF");
		}

		private TempFileCollection temps;
		private Assembly assembly;
		private string path;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			// at full trust
			temps = new TempFileCollection ();
			assembly = Assembly.GetExecutingAssembly ();
			path = assembly.Location;
		}

		[Test]
		[PermissionSet (SecurityAction.Deny, Unrestricted = true)]
		public void Deny_Unrestricted ()
		{
			CompilerResults cr = new CompilerResults (temps);
			Assert.IsNull (cr.CompiledAssembly, "CompiledAssembly");
			cr.CompiledAssembly = assembly;
			Assert.AreEqual (0, cr.Errors.Count, "Errors");
			Assert.IsNull (cr.Evidence, "Evidence");
			Assert.AreEqual (0, cr.NativeCompilerReturnValue, "NativeCompilerReturnValue");
			cr.NativeCompilerReturnValue = 1;
			Assert.AreEqual (0, cr.Output.Count, "Output");
			Assert.IsNull (cr.PathToAssembly, "PathToAssembly");
			cr.PathToAssembly = path;
			Assert.AreEqual (0, cr.TempFiles.Count, "TempFiles");
			cr.TempFiles = new TempFileCollection ();
		}

		[Test]
		[SecurityPermission (SecurityAction.PermitOnly, ControlEvidence = true)]
		public void Evidence_PermitOnly_Unrestricted ()
		{
			CompilerResults cr = new CompilerResults (temps);
			cr.Evidence = new Evidence ();
		}

		[Test]
		[SecurityPermission (SecurityAction.Deny, ControlEvidence = true)]
		[ExpectedException (typeof (SecurityException))]
		public void Evidence_Deny_Unrestricted ()
		{
			CompilerResults cr = new CompilerResults (temps);
			cr.Evidence = new Evidence ();
		}

		[Test]
		public void LinkDemand_No_Restriction ()
		{
			Type[] types = new Type[1] { typeof (TempFileCollection) };
			ConstructorInfo ci = typeof (CompilerResults).GetConstructor (types);
			Assert.IsNotNull (ci, ".ctor(TempFileCollection)");
			Assert.IsNotNull (ci.Invoke (new object[1] { temps }), "invoke");
		}

		[Test]
		[EnvironmentPermission (SecurityAction.Deny, Read = "Mono")]
		[ExpectedException (typeof (SecurityException))]
		public void LinkDemand_Deny_Anything ()
		{
			// denying anything results in a non unrestricted permission set
			Type[] types = new Type[1] { typeof (TempFileCollection) };
			ConstructorInfo ci = typeof (CompilerResults).GetConstructor (types);
			Assert.IsNotNull (ci, ".ctor(TempFileCollection)");
			Assert.IsNotNull (ci.Invoke (new object[1] { temps }), "invoke");
		}
	}
}
