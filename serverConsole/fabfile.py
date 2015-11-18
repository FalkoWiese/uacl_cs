#!/usr/bin/env python
# vim: set fileencoding=utf-8 :
from __future__ import print_function
import inspect
import logging
import datetime
import sys
import glob
import os
import shutil
import time
import re
import codecs
from urllib2 import URLError
# import win32api
from fabric.api import *
from fabric.colors import *
from fabric.main import main
from enum import Enum

try:
    from fabfile_local import *
except ImportError:
    pass

logging.basicConfig()

# some common settings
ms_build_path = r'C:\Windows\Microsoft.NET\Framework\v4.0.30319'
nunit_path = r'C:\Program Files (x86)\NUnit.org\nunit-console'
nuget_path = r'C:\Program Files (x86)\NuGet'

path_to_test_lib = r'.\TestServerConsole\bin\Debug'
server_solution = "serverConsole.sln"


@task
def all():
    clean()
    update_env()
    build()
    test()


@task
def update_env():
    update_packages(project_folder='serverConsole')


@task
def build():
    ms_build(solution=server_solution)


@task
def clean():
    ms_build(solution=server_solution, target="Clean")


@task
def test():
    nunit_test(path_to_dll=os.path.join(path_to_test_lib, "TestServerConsole"))


def nunit_test(path_to_dll):
    nunit = os.path.join(nunit_path, 'nunit3-console.exe')
    local('"{nunit}" {path_to_dll}.dll'.format(nunit=nunit, path_to_dll=path_to_dll))


def ms_build(solution, target='Build', more='/m'):
    ms_build = os.path.join(ms_build_path, 'MSBuild.exe')
    build_cmd = ms_build
    cmd_add = lambda cmd: build_cmd + ' {0}'.format(cmd) if cmd else build_cmd
    build_cmd = cmd_add(more)
    build_cmd = cmd_add('/t:' + target)
    build_cmd = cmd_add(solution)
    print('Perform a {0}'.format(target.lower()))
    return local(build_cmd)


def update_packages(project_folder):
    pm_path = os.path.join(nuget_path, 'nuget.exe')
    packages_file = os.path.join(project_folder, 'packages.config')
    local('"{pm}" install {pf}'.format(pm=pm_path, pf=packages_file))


# Main function call for the debugging
if __name__ == '__main__':
    sys.argv = ['fab', '-f', __file__, 'deploy']
    main()
