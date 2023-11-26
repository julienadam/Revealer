// --------------------------------------------------------------------------------------
// F# Markdown (Collections.fs)
// (c) Tomas Petricek, 2012, Available under Apache 2.0 license.
// Modified by Julien Adam for use in Revealer
// Removed unused code
// --------------------------------------------------------------------------------------

namespace FSharp.Collections

// --------------------------------------------------------------------------------------
// Various helper functions for working with lists
// that are useful when writing parsers by hand.
// --------------------------------------------------------------------------------------

module internal List =
    /// Iterates over the elements of the list and calls the first function for
    /// every element. Between each two elements, the second function is called.
    let rec iterInterleaved f g input =
        match input with
        | x :: y :: tl ->
            f x
            g ()
            iterInterleaved f g (y :: tl)
        | x :: [] -> f x
        | [] -> ()
