// Copyright 2021 Russian Post
// This source code is Russian Post Confidential Proprietary.
// This software is protected by copyright. All rights and titles are reserved.
// You shall not use, copy, distribute, modify, decompile, disassemble or reverse engineer the software.
// Otherwise this violation would be treated by law and would be subject to legal prosecution.
// Legal use of the software provides receipt of a license from the right holder only.

using System;
using System.Threading.Tasks;
using Tizen;
using Tizen.Security;

namespace FastQR
{
    public class UserPermission
    {
        private TaskCompletionSource<bool>? tcs;

        public async Task<bool> CheckAndRequestPermission(string privilege)
        {
            try
            {
                CheckResult result = PrivacyPrivilegeManager.CheckPermission(privilege);
                Log.Debug(Utility.LogTag, "State: " + result);
                switch (result)
                {
                    case CheckResult.Allow:
                        return true;
                    case CheckResult.Deny:
                        Log.Debug(Utility.LogTag, "In this case, health data is not available until the user changes the permission state from the Settings application.");
                        return false;
                    case CheckResult.Ask:
                        PrivacyPrivilegeManager.GetResponseContext(privilege).TryGetTarget(out PrivacyPrivilegeManager.ResponseContext context);
                        if (context == null)
                        {
                            return false;
                        }
                        tcs = new TaskCompletionSource<bool>();
                        context.ResponseFetched += PPMResponseHandler;
                        PrivacyPrivilegeManager.RequestPermission(privilege);
                        return await tcs.Task;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.Error(Utility.LogTag, "An error occurred. : " + e.Message);
                return false;
            }
        }

        private void PPMResponseHandler(object sender, RequestResponseEventArgs e)
        {
            if (e.cause == CallCause.Error)
            {
                Log.Error(Utility.LogTag, "Error in Request Permission");
                tcs?.SetResult(false);
                return;
            }

            switch (e.result)
            {
                case RequestResult.AllowForever:
                    Log.Debug(Utility.LogTag, "Response: RequestResult.AllowForever");
                    tcs?.SetResult(true);
                    break;
                case RequestResult.DenyForever:
                case RequestResult.DenyOnce:
                    Log.Debug(Utility.LogTag, "Response: RequestResult." + e.result);
                    tcs?.SetResult(false);
                    break;
            }
        }
    }
}
