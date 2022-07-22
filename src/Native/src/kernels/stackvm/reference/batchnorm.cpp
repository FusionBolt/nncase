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
#include "kernel_template.h"
#include <iostream>
#include <nncase/kernels/cpu/reference/runtime_types.h>
#include <nncase/kernels/kernel_utils.h>
#include <nncase/kernels/stackvm/ref_ops.h>
#include <nncase/runtime/runtime_op_utility.h>

using namespace nncase;
using namespace nncase::kernels::cpu::reference;

result<void> nncase::kernels::stackvm::reference::batchnorm(
    const float *input, const float *scale, const float *bias,
    const float *input_mean, const float *input_var, float *output,
    const dims_t &in_shape, const strides_t &in_strides,
    const strides_t &out_strides, float epsilon) {
    return apply(in_shape, [&](const dims_t &index) -> result<void> {
        auto c = index[1];
        const auto x = input[offset(in_strides, index)];
        output[offset(out_strides, index)] =
            (x - input_mean[c]) / std::sqrt(input_var[c] + epsilon) * scale[c] + bias[c];
        return ok();
    });
}