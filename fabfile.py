#!/usr/bin/env python
# vim: set fileencoding=utf-8 :

from __future__ import print_function
import logging
import os
from fabric.api import *
import shutil

try:
    from fabfile_local import *
except ImportError:
    pass

logging.basicConfig()

# some global settings
ms_build_path = r'C:\Windows\Microsoft.NET\Framework\v4.0.30319'
nunit_path = r'C:\Program Files (x86)\NUnit.org\nunit-console'
nuget_path = r'C:\Program Files (x86)\NuGet'

# project path settings
impl_projects = ['ServerConsole', 'UaclServer']
test_projects = ['Test{p}'.format(p=p) for p in impl_projects]
projects = impl_projects + test_projects
solution = "ServerConsole"


@task
def all():
    clean()
    update_env()
    build()
    test()


@task
def update_env():
    update_packages()


@task
def build():
    ms_build(solution=solution, config='Debug')


@task
def release():
    ms_build(solution=solution, config='Release')


@task
def clean():
    ms_build(solution=solution, target="Clean")
    for p in projects:
        for sf in ['obj', 'bin']:
            shutil.rmtree(path=os.path.join(p, sf), ignore_errors=True)


@task
def test():
    for t in test_projects:
        path_to_test_lib = os.path.join('.', t, 'bin', 'Debug')
        test_impl(path_to_dll=os.path.join(path_to_test_lib, t))


def test_impl(path_to_dll):
    nunit = os.path.join(nunit_path, 'nunit3-console.exe')
    local('"{nunit}" {path_to_dll}.dll'.format(nunit=nunit, path_to_dll=path_to_dll))


def ms_build(solution, target='Build', config=None, more='/m'):
    ms_build = os.path.join(ms_build_path, 'MSBuild.exe')
    build_cmd = ms_build
    cmd_add = lambda cmd: build_cmd + ' {0}'.format(cmd) if cmd else build_cmd
    build_cmd = cmd_add(more)
    build_cmd = cmd_add('/t:' + target)
    if config:
        build_cmd = cmd_add('/p:Configuration={config}'.format(config=config))
    build_cmd = cmd_add("{solution}.sln".format(solution=solution))
    local(build_cmd)


def update_packages():
    pm_path = os.path.join(nuget_path, 'nuget.exe')
    for p in projects:
        packages_config = os.path.join(p, 'packages.config')
        local('"{pm}" install -OutputDirectory packages {pf}'.format(pm=pm_path, pf=packages_config))
