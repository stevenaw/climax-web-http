/*
  Generic Node Module for handling release note parsing
*/
var fs = require('fs');
var path = require('path');

module.exports = function() {
  var versionRegexMultiline = /^###\s*([0-9]+\.[0-9]+\.[0-9]+)$/m,
    versionRegexGreedy = /^###\s*([0-9]+\.[0-9]+\.[0-9]+)$/g,
    newLineRegex = /[^\r\n]+/g;

  this.onError = null;

  function onFatalError(error) {
    if(this.onError) {
      this.onError(error);
    } else {
      throw new Error('No error handler defined! Unhandled error: ' + error);
    }
  }

  this.getAppVersion = function(releaseNotesStr, releaseNotesPath) {
    var versions = versionRegexMultiline.exec(releaseNotesStr);

    if (!versions) {
      onFatalError('Unable to locate version strings in ' + releaseNotesPath + '\n\nVersions should be headings with ### tags.  Eg. ### 1.3.5');
    }

    // versions[0] is the matched string, versions[1] is capture group only
    return versions[1];
  };

  this.getAppChangelog = function(lines, appVersion) {
    var changelog = [],
      captureInfo = false;

    for(var i=0; i<lines.length; i++) {
      if (versionRegexGreedy.test(lines[i])) {
        captureInfo = lines[i].indexOf(appVersion) > -1;
      } else if (captureInfo) {
        var changeLogLine = lines[i].trim();

        if(changeLogLine) {
          changelog.push(changeLogLine);
        }
      }
    }

    return changelog;
  };

  this.parseReleaseNotes = function(releaseNotesPath) {
    var releaseNotesStr,
      lines,
      version,
      changelog;

    releaseNotesStr = this.readReleaseNotes(releaseNotesPath);
    lines = releaseNotesStr.match(newLineRegex);
    version = this.getAppVersion(releaseNotesStr, releaseNotesPath);
    changelog = this.getAppChangelog(lines, version);

    return {
      version: version,
      changelog: changelog
    };
  };

  this.readReleaseNotes = function(releaseNotesPath) {
    var releaseNotesStr;

    releaseNotesPath = path.resolve(releaseNotesPath || './RELEASE_NOTES.md');

    try {
      releaseNotesStr = fs.readFileSync(releaseNotesPath, { encoding: 'utf8' });
    } catch (e) {
      onFatalError('Unable to find release notes file at ' + releaseNotesPath);
    }

    return releaseNotesStr;
  };

  this.prepareVersions = function(appVersion, buildCounter) {
    var versionParts,
      majorMinor,
      versions = {
        semver: appVersion,
        assembly: null,
        assemblyFile: null,
        patch: null
      };
    
    if (buildCounter.length > 3) {
      onFatalError('Build counter (' + buildCounter + ') has more than 3 digits');
    } else {
      // NuGet's sorting algorithm is string-based. This means 1.0.2 will come before 1.0.10.
      // To circumvent this problem we pad with zeros.  1.0.2 => 1.0.002
      versions.patch = ('000' + buildCounter).slice(-3);
    }

    // Extract major minor from the semantic version
    versionParts = versions.semver.split('.');
    majorMinor = versionParts[0] + '.' + versionParts[1];

    // .NET assembly version.
    // Note: To avoid dependency hell, always limit to minor version bumps
    versions.assembly = majorMinor + '.0.0';
    versions.assemblyFile = versions.semver + '.0';

    return versions;
  };

  return this;
}();