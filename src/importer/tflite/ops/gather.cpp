/* Copyright 2019-2021 Canaan Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#include "../tflite_importer.h"
#include <nncase/ir/ops/gather.h>
#include <nncase/ir/ops/convert.h>

using namespace nncase;
using namespace nncase::importer;
using namespace nncase::ir;

DEFINE_TFLITE_LOWER(GATHER)
{
    auto &input = get_tensor(op.inputs(), 0);
    auto &indices = get_tensor(op.inputs(), 1);
    auto &output = get_tensor(op.outputs(), 0);

    auto in_shape = get_shape(input.shape());
    auto indices_shape = get_shape(indices.shape());
    auto out_shape = get_shape(output.shape());

    auto &options = *op.builtin_options_as_GatherOptions();

    const auto in_type = to_data_type(input.type());
    const auto indices_type = to_data_type(indices.type());
    auto axis = options.axis();
    if (axis < 0)
    {
        axis = axis + static_cast<int32_t>(in_shape.size());
    }
    if(indices_type != dt_int32)
    {
        auto ga = graph_.emplace<gather>(in_type, in_shape, indices_shape, out_shape, axis);
        auto ct = graph_.emplace<convert>(indices_type, indices_shape, dt_int32);
        ga->indices().connect(ct->output());
        link_input_tensor(&ga->input(), op.inputs()->Get(0));
        link_input_tensor(&ct->input(), op.inputs()->Get(1));
        link_output_tensor(op.outputs()->Get(0), &ga->output());
    }
    else
    {
        auto ga = graph_.emplace<gather>(in_type, in_shape, indices_shape, out_shape, axis);
        ga->name(get_tensor(op.outputs(), 0).name()->string_view());

        link_input_tensor(&ga->input(), op.inputs()->Get(0));
        link_input_tensor(&ga->indices(), op.inputs()->Get(1));
        link_output_tensor(op.outputs()->Get(0), &ga->output());
    }
}