# Copyright 2019-2021 Canaan Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
"""System test: test slice"""
# pylint: disable=invalid-name, unused-argument, import-outside-toplevel
import pytest
import os
import tensorflow as tf
import numpy as np
import sys
import test_util


def _make_module(in_shape, begin, size):
    class SliceModule(tf.Module):
        def __init__(self):
            super(SliceModule).__init__()

        @tf.function(input_signature=[tf.TensorSpec(in_shape, tf.float32)])
        def __call__(self, x):
            return tf.slice(x, begin, size)
    return SliceModule()


cases = [
    ([3], [0], [1]),
    ([5], [1], [2]),
    ([6, 3], [1, 0], [2, 2]),
    ([6, 3, 3], [2, 2, 1], [3, 1, 2]),
    ([6, 3, 5, 5], [0, 1, 2, 2], [3, 1, 2, 3])
]


@pytest.mark.parametrize('in_shape,begin,size', cases)
def test_slice(in_shape, begin, size, request):
    module = _make_module(in_shape, begin, size)
    test_util.test_tf_module(request.node.name, module, [
                             'cpu', 'k210', 'k510'])


if __name__ == "__main__":
    pytest.main(['-vv', 'test_slice.py'])