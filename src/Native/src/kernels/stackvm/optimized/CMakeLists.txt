cmake_minimum_required (VERSION 3.13)

set(SRCS
         concat.cpp
         convolution.cpp
         slice.cpp
         dequantize.cpp
         resize_image.cpp
         gather.cpp
         gather_nd.cpp
         quantize.cpp
         onehot.cpp
        )
target_sources(kernels PRIVATE ${SRCS})