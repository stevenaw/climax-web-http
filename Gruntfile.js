/**
 * Climax Gruntfile for building & deploying NuGet packages
 */

'use strict';

var fs = require('fs');
var path = require('path');
var utilities = require('./BuildUtilities.js');

/**
 * Build script
 */
module.exports = function (grunt) {
  var branch = grunt.option('branch') || 'develop',
      buildCounter = grunt.option('buildCounter') || 0,
      solutionName = 'Climax.Web.Http',
      appSolution = grunt.option('lib.solution') || (solutionName + '.sln'),
      buildCOnfiguration = grunt.option('lib.configuration') || 'Release',
      releaseInfo;

      utilities.onError = grunt.fail.fatal;
      releaseInfo = utilities.parseReleaseNotes();

  function getNuspec() {
    var contents = '';

    contents += '<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">';
      contents += '<metadata>'
        contents += '<id>' + solutionName + '</id>';
        contents += '<version>' + releaseInfo.version + '</version>';
        contents += '<authors>filipw, climax-media</authors>';
        contents += '<owners>filipw, climax-media</owners>';
        contents += '<summary>A collection of add-ons for ASP.NET Web API</summary>';
        contents += '<licenseUrl>https://github.com/climax-media/climax-web-http/blob/master/LICENSE.txt</licenseUrl>';
        contents += '<projectUrl>https://github.com/climax-media/climax-web-http</projectUrl>';
        contents += '<iconUrl>https://avatars1.githubusercontent.com/u/4292694</iconUrl>';
        contents += '<requireLicenseAcceptance>false</requireLicenseAcceptance>';
        contents += '<description>A collection of add-ons for ASP.NET Web API</description>';
        contents += '<releaseNotes>' + releaseInfo.changelog.join('\r\n') + '</releaseNotes>';
        contents += '<copyright>Copyright 2015 Climax Media Inc</copyright>';
        contents += '<tags>webapi climax aspnet aspnetwebapi</tags>';

        contents += '<dependencies>';
          contents += '<group>';
            contents += '<dependency id="Microsoft.AspNet.WebApi.Core" version="5.2.3" />';
            contents += '<dependency id="Microsoft.AspNet.WebApi.Cors" version="5.2.3" />';
          contents += '</group>';
        contents += '</dependencies>';

      contents += '</metadata>';


      contents += '<files>';
        contents += '<file src="Climax.Web.Http.dll" target="lib\\net45" />';
        contents += '<file src="Climax.Web.Http.pdb" target="lib\\net45" />';
      contents += '</files>';
    contents += '</package>';

    return contents;
  }

  /**
   * Task configurations
   */
  grunt.initConfig({

    app: {
      versions: utilities.prepareVersions(releaseInfo.version, buildCounter),
      changelog: releaseInfo.changelog.join('\r\n'),
      branch: branch,
      buildCounter: buildCounter,

      // NuGet settings
      nuget: {
        exe: grunt.option('nuget.exe') || '.nuget/NuGet.exe',
        config: grunt.option('nuget.config') || '.nuget/NuGet.config',
        apiKey: grunt.option('nuget.apiKey'),
        pushSource: grunt.option('nuget.pushSource')
      },

      // Project-specific settings.
      lib: {
        solution: appSolution,
        nuspecFile: 'src/' + solutionName + '/bin/' + buildCOnfiguration + '/' + solutionName + '.nuspec',
        buildConfiguration: buildCOnfiguration,
        buildTargets: grunt.option('lib.buildTargets') || 'Rebuild',
        packagesBin: grunt.option('lib.packagesBin') || 'packages'
      }
    },

    nugetrestore: {
      dist: {
        src: 'src/' + solutionName + '/packages.config',
        dest: '<%= app.lib.packagesBin %>'
      }
    },

    nugetpack: {
      dist: {
        src: '<%= app.lib.nuspecFile %>',
        dest: 'build/packages/' + solutionName
      },
      options: {
        version: '<%= app.versions.semver %>',
        includeReferencedProjects: true,
        symbols: true,
        properties: 'Configuration=<%= app.lib.buildConfiguration %>',
        nugetExe: '<%= app.nuget.exe %>'
      }
    },

    msbuild: {
      dist: {
        src: ['<%= app.lib.solution %>'],
        options: {
          projectConfiguration: '<%= app.lib.buildConfiguration %>',
          targets: ['<%= app.lib.buildTargets %>']
        }
      }
    },

    nunit: {
      test: {
        files: {
          src: ['<%= app.lib.solution %>']
        }
      },
      options: {
      	path: 'packages/NUnit.Runners.2.6.4/tools'
      }
    },

   /**
   * This replace will:
   * 1) remove any existing AssemblyInformationalVersion (wtih empty lines)
   * 2) replace AssemblyVersion with build specific AssemblyVersion
   * 3) replace AssemblyFileVersion with build specific AssemblyFileVersion and AssemblyInformationalVersion
   * important: we want the regex replacements to be done in order
   */
    replace: {
      dist: {
        options: {
          patterns: [{
            match: /\[assembly: AssemblyInformationalVersion\("([0-9]+(\.([0-9]+|\*)){1,3}(\-(alpha|beta)[0-9]{3})?|undefined)"\)]+((\s\n){1,})?/g,
            replacement: '',
          },{
            match: /AssemblyVersion\("([0-9]+(\.([0-9]+|\*)){1,3}|undefined)"\)/g,
            replacement: 'AssemblyVersion("<%= app.versions.assembly %>")'
          },{
            match: /\[assembly: AssemblyFileVersion\("([0-9]+(\.([0-9]+|\*)){1,3}(\-(alpha|beta)[0-9]{3})?|undefined)"\)]/g,
            replacement: '[assembly: AssemblyFileVersion("<%= app.versions.assemblyFile %>")]\r\n[assembly: AssemblyInformationalVersion("<%= app.versions.semver %>")]'
          }],
          force: true,
          preserveOrder: true 
        },
        files: [
          {expand: true, src: ['**/AssemblyInfo.cs'], dest: 'src/', cwd: 'src/'}
        ]
      }
    },

    clean: {
      dist: {
        src: ['build/packages']
      }
    }
  });

  grunt.loadNpmTasks('grunt-nuget');
  grunt.loadNpmTasks('grunt-msbuild');
  grunt.loadNpmTasks('grunt-replace');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-nunit-runner');

  /**
   * Task definitions
   */

  //run unit tests
  grunt.registerTask('test', ['nunit']);

  // Restore NuGet dependencies
  grunt.registerTask('restore', ['nugetrestore']);

  // Build solution
  grunt.registerTask('build', ['replace', 'msbuild', 'nunit']);

  // Clen, build and create NuGet package
  grunt.registerTask('pack', ['clean', 'makepackage']);

  // Build and create NuGet package
  grunt.registerTask('makepackage', ['build', 'makenuspec', 'nugetpack', 'cleannuspec']);

  // Make nuspec file
  grunt.registerTask('makenuspec', function() {
    var done,
        app = grunt.config.get('app');

    done = this.async();

    var nuspecContents = getNuspec();

    fs.writeFile(app.lib.nuspecFile, nuspecContents, function (err) {
      if (err) {
        grunt.fail(err);
      } else {
        grunt.log.writeln('Nuspec file created at ' + app.lib.nuspecFile);
      }

      done();
    });
  });

  // Clean nuspec file
  grunt.registerTask('cleannuspec', function() {
    var done,
        app = grunt.config.get('app');

    grunt.task.requires('makenuspec');

    done = this.async();

    fs.unlink(app.lib.nuspecFile, function (err) {
      if (err) {
        grunt.fail(err);
      } else {
        grunt.log.writeln('Nuspec file cleaned at ' + app.lib.nuspecFile);
      }

      done();
    });
  });

  // Helper task to log all custom settings being used
  grunt.registerTask('logSettings', function() {
    grunt.log.writeln('Build settings: ');
    grunt.log.write(JSON.stringify(grunt.config('app'), null, '  '));
  });

  // Run everything in sequence
  grunt.registerTask('deploy', [
    'logSettings',
    'clean',
    'restore',
    'makepackage'
  ]);
};