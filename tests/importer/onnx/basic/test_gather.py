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
"""System test: test gather"""
# pylint: disable=invalid-name, unused-argument, import-outside-toplevel

import pytest
import onnx
from onnx import helper
from onnx import AttributeProto, TensorProto, GraphProto
from test_runner import OnnxTestRunner
import numpy as np

def result_shape(p_shape, i_shape, axis=0):
    return p_shape[:axis] + i_shape + p_shape[axis + 1:]

def _make_module(in_shape, indices, axis):
    input = helper.make_tensor_value_info('input', TensorProto.FLOAT, in_shape)
    i_shape = list(np.array(indices).shape)
    indices = helper.make_tensor('indices', TensorProto.INT32, np.array(indices).shape, np.array(indices).flatten().tolist())
    output = helper.make_tensor_value_info('output', TensorProto.FLOAT, result_shape(in_shape, i_shape, axis))
    initializers = []
    initializers.append(indices)

    node = onnx.helper.make_node(
        'Gather',
        inputs=['input', 'indices'],
        outputs=['output'],
        axis=axis
    )
    graph_def = helper.make_graph(
        [node],
        'test-model',
        [input],
        [output],
        initializer=initializers
    )

    return helper.make_model(graph_def, producer_name='kendryte')

in_shapes_indices_dim = [
    ([3, 5], [[0, 1], [1, 2]], 1),
    ([5, 4, 3, 2], [[0, 1, 3], [1, 3, 2]], 0),
    ([5, 4, 3, 2], [[0, 1, 3], [1, 3, 2]], 1)
]

@pytest.mark.parametrize('in_shape,indices,dim', in_shapes_indices_dim)
def test_gather(in_shape, indices, dim, request):
    model_def = _make_module(in_shape, indices, dim)
    runner = OnnxTestRunner(request.node.name)
    model_file = runner.from_onnx_helper(model_def)
    runner.run(model_file)


if __name__ == "__main__":
    pytest.main(['-vv', 'test_gather.py'])
