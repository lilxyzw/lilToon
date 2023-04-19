#if UNITY_EDITOR
using System;
using System.Text.RegularExpressions;

namespace lilToon
{
    public class SemVerParser {
        public int major;
        public int minor;
        public int patch;
        public string prerelease;
        public string build;

        public SemVerParser(string versionString, bool isForced) {
            // https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
            var pattern = @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
            var pattern2 = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)";
            var regex = new Regex(pattern);
            var regex2 = new Regex(pattern2);
            var match = regex.Match(versionString);
            var match2 = regex2.Match(versionString);
            if (match.Success) {
                major = int.Parse(match.Groups[1].Value);
                minor = int.Parse(match.Groups[2].Value);
                patch = int.Parse(match.Groups[3].Value);
                prerelease = match.Groups[4].Success ? match.Groups[4].Value : null;
                build = match.Groups[5].Success ? match.Groups[5].Value : null;
            } else if (match2.Success) {
                major = int.Parse(match2.Groups[1].Value);
                minor = int.Parse(match2.Groups[2].Value);
                patch = int.Parse(match2.Groups[3].Value);
                prerelease = null;
                build = null;
            } else if(isForced) {
                major = 999999;
                minor = 999999;
                patch = 999999;
            } else {
                throw new ArgumentException("Invalid semver string: " + versionString);
            }
        }

        public SemVerParser(string versionString) {
            // https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
            var pattern = @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
            var regex = new Regex(pattern);
            var match = regex.Match(versionString);
            if (match.Success) {
                major = int.Parse(match.Groups[1].Value);
                minor = int.Parse(match.Groups[2].Value);
                patch = int.Parse(match.Groups[3].Value);
                prerelease = match.Groups[4].Success ? match.Groups[4].Value : null;
                build = match.Groups[5].Success ? match.Groups[5].Value : null;
            } else {
                throw new ArgumentException("Invalid semver string: " + versionString);
            }
        }

        public int CompareTo(SemVerParser other) {
            if (other == null) {
                return 1;
            }

            if (major != other.major) {
                return major.CompareTo(other.major);
            }

            if (minor != other.minor) {
                return minor.CompareTo(other.minor);
            }

            if (patch != other.patch) {
                return patch.CompareTo(other.patch);
            }

            if (prerelease == null && other.prerelease != null) {
                return 1;
            } else if (prerelease != null && other.prerelease == null) {
                return -1;
            } else if (prerelease != null && other.prerelease != null) {
                int cmp = string.Compare(prerelease, other.prerelease);
                if (cmp != 0) {
                    return cmp;
                }
            }

            if (build == null && other.build != null) {
                return 1;
            } else if (build != null && other.build == null) {
                return -1;
            } else if (build != null && other.build != null) {
                int cmp = string.Compare(build, other.build);
                if (cmp != 0) {
                    return cmp;
                }
            }

            return 0;
        }

        public static bool operator ==(SemVerParser v1, SemVerParser v2) {
            if (ReferenceEquals(v1, v2)) {
                return true;
            }
            if (ReferenceEquals(v1, null) || ReferenceEquals(v2, null)) {
                return false;
            }
            return v1.CompareTo(v2) == 0;
        }
        public static bool operator !=(SemVerParser v1, SemVerParser v2) { return !(v1 == v2); }
        public static bool operator < (SemVerParser v1, SemVerParser v2) { return v1.CompareTo(v2) < 0; }
        public static bool operator <=(SemVerParser v1, SemVerParser v2) { return v1.CompareTo(v2) <= 0; }
        public static bool operator > (SemVerParser v1, SemVerParser v2) { return v1.CompareTo(v2) > 0; }
        public static bool operator >=(SemVerParser v1, SemVerParser v2) { return v1.CompareTo(v2) >= 0; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
#endif